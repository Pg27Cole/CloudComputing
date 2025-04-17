# ASSIGNMENT ONE CLOUD COMPUTING
#### Raf housing Saving

## **NOTE: I will send TA Spencer the .envs file through discord**


## How to setup saving 
Paste the provided .env file into the raf_housing Server backend directory

Ensure you have run the command 'npm start' using the command line within the backend directory. Once this command is run you should see a console message confirming that the server is running on port 3000

## When is data saved
Data is saved both locally and to the server when the player clicks the "save" button

## Where is the data saved
The local save data is saved to the persistent data path. the simulated cloud saves are saved in the "Raf_housing Server Backend" folder  

**_________________________________________**
### **-------_SUPER_COLE_--------**
**_________________________________________**
                 
# ASSIGNMENT TWO CLOUD COMPUTING
#### Raf house telemetry

## How to setup telemetry
- Paste the provided .env file into the raf_housing Server backend directory  
- Ensure you run the command npm start using the command line within the backend directory. Once this command has been run you should see a console message confirming that the server is running on port 3000  

## When is telemetry data saved
Data is saved both locally and to the server when the player clicks the "save" button

## Where is the telemetry data saved
The simulated cloud saves are saved in the "Raf_housing Server Backend" folder under the name "events.json" while as the locally saved telemetry data is saved in the persistent data path with the name starting with the username, and ending with local_Events.json


## Additional telemetry data I stored
I stored the number of times that the user changed the seed value by using the slider. This might be useful to determine how valued the slider is to the users. 

**_________________________________________**  
### **----_SUPER__DUPER_COLE_-----**
**_________________________________________**  

# ASSIGNMENT THREE CLOUD COMPUTING
#### Raf housing log in system 

## How to setup logging in  
Ensure you followed all the steps for the previous assignment submissions and it should be all good 👍

## When is the token saved
The token is saved upon successful login and it expires thirty seconds seconds afterwards. It is so short for the purposes of testing 

## Where is the token saved
The token is saved to a file called "token.txt" in the persistent data path

