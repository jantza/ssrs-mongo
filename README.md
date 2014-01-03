## SSRS-Mongo

Application to pull SQL Server Reporting Services (SSRS) usage statistics from a reporting SQL Server instance into MongoDB.

### Why?
 
SSRS keeps report execution data in a relational table with all of the report parameter information in a single text field.  The report parameter information can be very valuable when looking into report usage, but because it's non-relational data it's not easily queryable with SQL.  

This application will parse the report parameter data into BSON Documents for MongoDB to store along with the standard execution info.  This allows you to write Mongo queries that can use the report parameter information.

### How to use it

Update the app.config connection string settings to point to the SQL Server Reporting Services database and the MongoDB instance that you'd like to store the information in.  

If there are specific report parameters that you'd like to exclude before running, make sure to update the `ParametersToIgnore` array in `Program.cs`.  By default SSRS-Mongo will try to find the last execution time stored in MongoDB and grab all report executions since.  If no documents exist in Mongo, then the `MaxDaysBack` value will be used to calculate how far back for retrieving records.

If you'd like to use a different database or collection name for storing documents use the appropriate settings in app.config.

 
