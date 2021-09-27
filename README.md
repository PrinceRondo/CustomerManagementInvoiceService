# CustomerManagementInvoiceService
The application will be sending invoice upload url to the customer
This application (windows service) helps in sending invoice url to customers that are due for it

Application Installation
You will be running the command below to install and uninstall the services base on the file location
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe" "C:\inetpub\wwwroot\Email\EmailNotificationService.exe"

"C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe" -u "C:\inetpub\wwwroot\Email\EmailNotificationService.exe"


How to use the Application

programming Language: C#
Database: MSSQL
ORM: Entity Framework
SMTP: Gmail

The application will be running once in a month to check the database for records that have not been sent to the customer. It will be updating
 the database, changing the sending status and setting the link expiration time after sending the link through email. A log file is situated in
the project folder for all the activities.
