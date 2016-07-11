# azure-solutions-digital-marketing-reference-implementation
Reference implmentation for the Azure Digital Marketing Solution. This example provides an initial framework for the development of a multi-channel digital marketing solution that is customizable and extensible.
## Requirements
-   Active Azure Subscription

-   Visual Studio 2015 Update 3

-   Azure SDK and Tools 2.9 for Visual Studio 2015

## Running this web app locally
1.  Clone the repo

1.  Open the **AzureKit - Server Only** solution file in Visual Studio 2015.

1.  Perform a Build to restore NuGet packages.

1.  Press F5.

1.  Accept the request to enable SSL.

1.  You should see the site home page.

## Deploy the web app to Azure
In order to deploy to Azure using the included ARM template, you will need an ARM accessible storage account. In addition, you’ll want to create a custom Azure AD tenant specifically for this app.

1.  In the **AzureKit.Deployment** project, expand the **Templates** folder.

1.  Open **AzureKit.Developer.parameters.json**.

1.  Change the **UniqueNamePrefix** **value** to something other than **azKit**.

1.  Save your change.

1.  Right click **AzureKit.Deployment** and select **Deploy | New Deployment**.

1.  In the **Deploy Resource Group**, log in with your Azure credentials.

1.  Select your desired **Subscription**.

1.  In the **Resource group** combo-box, choose the **Create New** option, or select an existing one.

1.  From the **Deployment template** combo-box, select the **azurekit.developer.json** item.

1. In the **Deployment** template combo-box, select the **azureKit.developer.parameters.json item**.

1. For the **Artifact storage account**, select your desired storage account.

1. When ready, click **Deploy** to start the process. Monitor progress in the Visual Studio Output window. Once the deployment is done you can see the new objects created by accessing portal.azure.com and examine your **Resource Group**.

1. Next you need to deploy the web site. Right click on the **AzureKit** project and select **Publish**.

1. In the **Publish** dialog, select **Microsoft Azure App Service** for the **publish target**.

1. In the **App Service** dialog, select the **Azure Subscription** you used with your ARM template and then the **Resource Group**.

1. Finally expand your Resource Group in the list of available items and select the App Service that matches the **UniqueNamePrefix** you defined earlier.

1. Click **OK** when ready.

1. Back in the **Publish** dialog, click **Next** on the **Connection** page.

1. On the **Settings** page ***remove*** the check next to **Enable Organizational Authentication**. You will do this later in the Azure Portal.

1. Click **Next**.

1. Finally on the **Preview** page, click **Publish**. Once Visual Studio finishes the web deploy, it will open your default web browser to your site’s home page.

1. Access the legacy Azure Portal at <http://manage.windowsazure.com> to configure Azure Active Directory (Azure AD) for your application.

   > This is the old Azure management portal which you still need to use in order to manage some resources that have note moved to the new Azure Portal.

1.  Scroll down in the left navigation pane and select the Active Directory option.

     <img src="./media/image1.png" > 

1.  Click the + NEW button at bottom.

     <img src="./media/image2.png" >

1.  Select **APP SERVICES | ACTIVE DIRECTORY | DIRECTORY | CUSTOM CREATE**.

     <img src="./media/image3.png" >

1.  Provide a **Name** for your directory.

1.  Provide a unique **Domain Name**.

1.  Select the appropriate **Country or Region**.

1.  ***Do not*** check **This is a B2C directory**.

     <img src="./media/image4.png" >

1.  Click the **Check Mark**.

1.  Select your new directory.

1.  Click the **Domains** tab to view the domains associated with your directory.

     <img src="./media/image5.png" >

1.  You will see one default domain associated with the directory. Make note of this directory name as you will need it later for the web.config value **ida:Domain**.

     <img src="./media/image6.png" >

1.  Click on the **Applications** tab.

1.  Click the **Add** button in the bottom toolbar area to start the process of creating a new application.

     <img src="./media/image7.png" >

1.  When prompted, choose to **Add an application my organization is developing**.

     <img src="./media/image8.png" >

1.  Enter a **Name** for your application and leave the default selection for a **Web Application / Or Web API** selected.

     <img src="./media/image9.png" >

1.  Then click the right arrow button to move to the next step.

1.  For the **Sign-on URL** enter [**https://localhost:44300/**](https://localhost:44300/).

    > **Note** This is the address your application will be using when developing locally. When you deploy to Azure you can change this value in the portal or setup a separate application for the instance when it runs in Azure (recommended).

1.  Enter a unique URI in the **App ID Uri** field for the application such as you company domain name and the application name. This is a unique logical identifier for your app.

     <img src="./media/image10.png" >

    > **Note** Because the App ID URI is a logical identifier, it does not need to resolve to an Internet address.

1.  Click the check mark button to complete the application setup.

1.  Once Azure has created the application, select the **Configure** tab and scroll down to find the **Client ID**.

     <img src="./media/image11.png" >

1.  Copy the Client ID value using the button next to the field and save it somewhere You will use this as the value for the **ida:ClientId** in web.config.

1.  Scroll down the page. In the Single sign-on section, you’re going to add a Reply URL.

1.  Open a new browser tab if necessary and navigate to <http://portal.azure.com> and location your Resource Group.

1.  In the Resource Group, find your web app’s App Service and select it.

     <img src="./media/image19.png" width="366" height="34" />

1.  In the Web App Essentials section, copy the URL.

1.  Return to your browser window where you’re editing the Azure AD settings and add the URL you just copied as a Reply URL but change the protocol from HTTP to HTTPS.

1.  Click the **Save** button at the bottom.

     <img src="./media/image14.png" >

1.  Click the bottom toolbar button labeled **View Endpoints**.

     <img src="./media/image16.png" >

1.  Copy the tenant ID from any of the URLs provided. The tenant ID will be the GUID/Unique identifier immediately following the login domain. This will be used for the **ida:TenantId** in web.config.

    > **Note** You may need to use the copy button to copy the entire URL, then paste into a text editor to selectively copy out just the tenant ID.

1.  Close the **App Endpoints** dialog. Now you will configure users.

1.  In the management portal, click the “back” arrow to return to the directory tenant page.

     <img src="./media/image17.png" >

1.  Click the **Users** tab to view users for the tenant.

    > **Note** Make sure you are not on the tab of the same name for the application as that is for specific assignment of users to allow them access to the application.

     <img src="./media/image18.png" >

1.  Take note of the existing user. It will be the account you’re using to manage the directory. If you plan to test with this users, you can move on to finish configuring the application in Visual Studio.

1.  Click the **Add User** button in the bottom toolbar to add a new user.

1.  You can choose to add a new user (that will be covered here) or add other users with existing Microsoft Accounts or from other Azure Active Directory instances, and even partner organizations. For this example, choose **new user in your organization**.

1.  Enter a user name unique to this directory and move to the next dialog in the wizard.

1.  Enter values for the names and select **User** for this example.

    > **Note** You are creating a user in this directory. If this is your company directory or a production directory, be careful who you add as you may be giving them rights to your organization, applications, or data.

1.  Make sure to only choose **User** for the role as this is the role in the organization, not your application.

1.  Move to the next dialog in the wizard and click the button to get the temporary password.

    > **IMPORTANT** Copy this value as it will not be presented again and you will need it to login to the application. You will have to change the password on first login. Note that you can change the password at a later time from the management portal if you forget it.

1.  Complete the wizard.

## About the code
Coming soon...
## More information
Visit [**https://azure.microsoft.com/en-us/documentation/**](https://azure.microsoft.com/en-us/documentation/) for more information. 
