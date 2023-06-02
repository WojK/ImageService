# Image Service

The aim of this project was to create web application concentrated on security aspects.

Project consists of .Net WebApi and minimalistic React client. 

Implemented mechanisms and functions:
* JSON Web Token authorization
* Password storage protected by hash, salt and pepper
* Informing the user about new connections to his account (using cookies with device-token)
* The ability to place images on the server that are available privately or for specific users
* The ability to change the password
* Email confirmation after registration
* The ability to regain access in case of password loss (by sending email with link-token)
* CSRF protection (by architecture)
* Monitoring the number of failed login attempts
* Progressively adding a delay when verifying the password
* Informing the user about the quality of his password (his entropy)
* Checking the resistance of the new password to dictionary attacks (by searching some dictionary with passwords)
* Verification of the security of stored graphic files, by scanning uploaded files by containerized antivirus software 
