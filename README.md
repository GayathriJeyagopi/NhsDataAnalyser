# NhsDataAnalyser
Analyses Nhs Data and produces Report
-------------------------------------

NhsDataAnalyser is a console application which analyses the Nhs Practises data and Prescriptions data which are in the form of CSV files.
The sample data can be downloaded from the below location respectively,<br />

http://datagov.ic.nhs.uk/T201202ADD%20REXT.CSV <br />
http://datagov.ic.nhs/T201109PDP%20IEXT.CSV <br />

The CSV files has to be in a local directory. This console application currently does not support reading from the web. <br />
If you have already opened the CSV files in MS excel, please make sure that the excel sheets are closed. The MS excel application locks the files and does not allow even just reading.

Features:<br />
---------<br />

1. User can either type the file name with full path or drag and drop the file from the folder browser. If the user has entered a invalid file name,
he will be prompted to enter the correct file name again by pressing ENTER key, or any other key to quit the application. <br />
  
2. Once given the valid CSV files, the application answers the below queries, <br />
    
      1. How many practises are in the city of London? <br />
      2. What was the average actual cost all peppermint oil prescriptions? <br />
      3. Which 5 post codes have the have the highest actual spend and how much each spend in total? <br />
      4. For each region of England (North East, South West, etc.) <br />
       a) What was the average price per prescription of Flucloxacillin (excluding Co-Fluampicil)? <br />
       b) How much did this vary from the national mean? <br />
      5. How many practises are in a SHA region Q30, North East region <br />
    
  Note on File Parser: The File parser works only for CSV files and also the columns and headers should be in the specified format as in the sample files in the above location.<br />
  There is no validation on the format of CSV files currently. However this feature can be further extended. <br />
  
3. New queries can be added by any developer by providing a Query (aka Command) and a Query handler implementation. <br />
   
4. Testing: Unit testing and Component Testing are written using Nunit and Moq.<br />
      Component Testing: The idea of component testing is to test the component by using all real objects but mocking the external dependencies like low
      level classes, here in this case, Reading the file using System.File is an mock object. All other are real objects of NHSDataAnalyser component. <br />
      Please note Nunit 2.6.4 is used , the latest 3.4 does not support running tests from Nunit GUI Runner, otherwise the tests should also run (compatible) on the latest Nunit 3.4
      
Terminologies: <br />
---------------------
1. SHA - Strategic Health Authority
2. BNF - British National Formulary
3. PCT - Primary Care Trusts
4. Act - Actual Cost
5. NIC - Net Ingredient Cost - The cost before discount, does not include any dispensing cost or fees. Source: http://www.nhsbsa.nhs.uk/PrescriptionServices/2122.aspx
     
  

