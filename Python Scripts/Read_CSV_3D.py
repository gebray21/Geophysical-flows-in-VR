# -*- coding: utf-8 -*-
"""
Created on Sat Jun 17 19:09:56 2023

@author: gebrayha
"""
# -*- coding: utf-8 -*-
"""
Created on Sat Jun 17 00:41:41 2023

@author: gebrayha
"""
import os
import numpy as np
import pandas as pd
import geopandas as gpd 
from scipy.interpolate import griddata
import rasterio
import shutil
from rasterio.transform import from_origin
from rasterio.crs import CRS
import tkinter as tk
from tkinter import filedialog
from ConcaveHull import ConcaveHull
from shapely.geometry import Polygon, Point

#%%
#offsetX =612907.50
#offsetY =6658839.0
#rRes = 2 # cell size
#%%


#function to look for the csv files
def find_csv_filenames(path_to_dir, suffix=".csv"):
    filenames = os.listdir(path_to_dir)
    return [ filename for filename in filenames if filename.endswith(suffix) ]


#%%
def write_Grid(output,grid,rasterCrs,transform):
    grid = grid.astype(np.int32)
    Raster = rasterio.open(output,
                                'w',
                                driver='AAIGrid',  #AAIGrid for .asc, GTiff for .tiff
                                height=grid.shape[0], #rows
                                width=grid.shape[1],  #columns
                                count=1,
                                dtype=grid.dtype,
                                crs=rasterCrs,
                                transform=transform,
                                decimal_precision=3,
                                nodata = -9999)
    Raster.write(grid,1)
    Raster.close()

#%%

WKT = 'PROJCS["ETRS_1989_UTM_Zone_32N",\
        GEOGCS["GCS_ETRS_1989",\
        DATUM["D_ETRS_1989",SPHEROID["GRS_1980",6378137.0,298.257222101]],\
        PRIMEM["Greenwich",0.0],\
        UNIT["Degree",0.0174532925199433]],\
        PROJECTION["Transverse_Mercator"],\
        PARAMETER["False_Easting",500000.0],\
        PARAMETER["False_Northing",0.0],\
        PARAMETER["Central_Meridian",9.0],\
        PARAMETER["Scale_Factor",0.9996],\
        PARAMETER["Latitude_Of_Origin",0.0],UNIT["Meter",1.0]]'   
      
rasterCrs = CRS.from_wkt(WKT,morph_from_esri_dialect=False)
#rasterCrs = CRS.from_epsg(32632)
rasterCrs.data

#%%

# Function to browse the original folder
def browse_original_folder():
    global dir_with_csvs
    #original_folder_path = filedialog.askdirectory()
    dir_with_csvs = filedialog.askdirectory()
    print("Folder containing the CSV files:", dir_with_csvs)

# Function to browse the destination folder
def browse_destination_folder():
    global dir_with_ascs
    #destination_folder_path = filedialog.askdirectory()
    dir_with_ascs = filedialog.askdirectory()
    print("Destination Folder Path:", dir_with_ascs)
    
    
#%%
# Function to modify the CSV files
def read_csv_files():
    # Check if the original and destination folder paths are selected
    if 'dir_with_csvs' not in globals() or 'dir_with_ascs' not in globals():
        print("Please select the original and destination folders first.")
        status_label.config(text="Please select the original and destination folders first.")
        return
    
    print("Reading CSV files...")
    
    # Get the the offset values
    offsetX = float(X0_entry.get())
    offsetY = float(Y0_entry.get())
    rRes = float(Res_entry.get())
    print("X offset:", offsetX)
    print("Y offset:", offsetY)
    print("Resolution or cell size:", rRes)
    
    #working directly should be dir_with_csvs otherwise it won't read it
    os.chdir(dir_with_csvs)
    
    #folder creation (in order to meant breach api terminology)
    folder_list=[dir_with_ascs+'/Terrain File',dir_with_ascs+'/Elevation',dir_with_ascs+'/Velocity_X',dir_with_ascs+'/Velocity_Y',dir_with_ascs+'/shapefiles'] 
    for a in folder_list:
        dir = a
        if not os.path.exists(dir):
            os.makedirs(dir)
        else:
            shutil.rmtree(dir)           
            os.makedirs(dir)
            
    #Access the csv files 
    csvfiles = find_csv_filenames(dir_with_csvs)
    if not csvfiles:
            status_label.config(text="No CSV files found in the selected folder.")
            return    
    
    
    fn0 =csvfiles[0] # the initial file for boundary determination
    df0 =pd.read_csv(fn0,usecols=[0,1,2])
    df0 = df0.dropna() #delete missing data rows
    df0.columns =['x0','y0','z0']
    df0['x0'] = df0['x0'] + offsetX
    df0['y0'] = df0['y0'] + offsetY
    
    status_label.config(text="Reading CSV files...")  
    window.update_idletasks()  # Update the GUI to show the progress message     
    # Iterate over each CSV file
    for i, fn in enumerate(csvfiles):
        status_label.config(text="Reading CSV files... Please wait!")
        window.update_idletasks()  # Update the GUI to show the progress message
        df =pd.read_csv(fn,usecols=[0,1,2,3,4,5,6])
        df = df.dropna() #delete missing data rows
        df.describe()
        #Rename Columns
        df.columns =['x','y','z','u','v','w','alpha']
        df['x'] = df['x'] + offsetX
        df['y'] = df['y'] + offsetY   
        X0 = np.arange(df.x.min(),df.x.max()+rRes,rRes)
        Y0 =np.arange(df.y.max(),df.y.min()-rRes,-rRes)
        gridX0,gridY0 = np.meshgrid(X0, Y0)
        transformGrid  = from_origin(gridX0[0][0], gridY0[0][-1]+rRes, rRes, rRes)            
        point = list(zip(df.x,df.y)) #used for interpoplation
        points = df.to_numpy() #points as array for creating polygons
        points2d= points[:,[0,1]] #picks only x and y coordinates
        Pts = np.column_stack((gridX0.ravel(), gridY0.ravel())) # points in structured array
        #%%
        ch = ConcaveHull() #call concaveHull function
        ch.loadpoints(points2d)
        ch.calculatehull()
        ch.loadpoints(points2d)
        ch.calculatehull()
        boundary_points = np.vstack(ch.boundary.exterior.coords.xy).T
        polygon = Polygon(boundary_points)
        gdr =gpd.GeoDataFrame(index=[0], crs=WKT, geometry=[polygon]) #create boundary polygon
        #%% fetch the values to be interpolated
        z = df.z.values
        u =df.u.values
        v = df.v.values
        u.dtype =z.dtype
        v.dtype=z.dtype
        #%% clip the grids using the polygon
        # Create a mask for points inside the polygon
        mask = np.array([polygon.contains(Point(point)) for point in Pts])
        # Filter grid points inside the polygon
        grid_points_inside_polygon = Pts[mask]
        # Get the indices of grid points inside the polygon
        indices_inside_polygon = np.where(mask)[0]
        # Filter grid points inside the polygon
        grid_points_inside_polygon = Pts[indices_inside_polygon]    
         #%% interpolation in side the polygon
        #gridZ = griddata(point, z, (gridX0,gridY0), method='nearest', fill_value=-9999.0) #cubic, nearest, linear
        #gridU = griddata(point, u, (gridX0,gridY0), method='nearest',fill_value=-9999.0) #cubic, nearest, linear
        #gridV = griddata(point, v, (gridX0,gridY0), method='nearest',fill_value=-9999.0) #cubic, nearest, linear
        gridZ = griddata(point, z, grid_points_inside_polygon, method='nearest', fill_value=-9999.0) #cubic, nearest, linear
        gridU = griddata(point, u, grid_points_inside_polygon, method='nearest',fill_value=-9999.0) #cubic, nearest, linear
        gridV = griddata(point, v, grid_points_inside_polygon, method='nearest',fill_value=-9999.0) #cubic, nearest, linear
        #%% mapp the clipped values to the original grid
        
        # Create an array filled with -9999
        z_values = np.full_like(gridX0, -9999.000, dtype=np.float64)
        u_values = np.full_like(gridX0, -9999.000, dtype=np.float64)
        v_values = np.full_like(gridX0, -9999.000, dtype=np.float64)
        
        # Fill the array with the interpolated values for points inside the polygon
        z_values.ravel()[indices_inside_polygon] = gridZ
        u_values.ravel()[indices_inside_polygon] = gridU
        v_values.ravel()[indices_inside_polygon] = gridV
        
        #%%
        # filenames and folder paths - can be changed as desired...
        time= f'{i}'
        
        out_z = dir_with_ascs+'/Elevation/'+'Flow_Elevation_'+time+'.asc'
        out_u = dir_with_ascs+'/Velocity_X/' +'Velocity_x_'+time+'.asc'
        out_v = dir_with_ascs+'/Velocity_Y/' +'Velocity_y_'+time+'.asc'
        out_poly = dir_with_ascs+'/shapefiles/'+'polygon_'+time+'.shp'
        #write asc   
        write_Grid(out_z,z_values,rasterCrs,transformGrid)
        write_Grid(out_u,u_values,rasterCrs,transformGrid)        
        write_Grid(out_v,v_values,rasterCrs,transformGrid) 
        #write shapefile
        gdr.to_file(out_poly)
        

    status_label.config(text="Reading CSV files completed")
    
# Create a Tkinter GUI window
window = tk.Tk()
# Create a button to browse the original folder
original_folder_button = tk.Button(window, text="Browse Original Folder", command=browse_original_folder)
original_folder_button.pack()

# Create a button to browse the destination folder
destination_folder_button = tk.Button(window, text="Browse Destination Folder", command=browse_destination_folder)
destination_folder_button.pack()

# Create text entry fields for numerical inputs
X0_label = tk.Label(window, text="X offset:")
X0_label.pack()
X0_entry = tk.Entry(window)
X0_entry.pack()

Y0_label = tk.Label(window, text="Y offset:")
Y0_label.pack()
Y0_entry = tk.Entry(window)
Y0_entry.pack()

Res_label = tk.Label(window, text="Resolution:")
Res_label.pack()
Res_entry = tk.Entry(window)
Res_entry.pack()

# Create a button to execute the CSV modification process
modify_csv_button = tk.Button(window, text="Read CSV Files", command=read_csv_files)
modify_csv_button.pack()


# Status label
status_label = tk.Label(window, text="")
status_label.pack()
# Run the Tkinter event loop
window.mainloop()


        
