# Lab: VSTS Release Management

**This lab assumes you've completed the VSTS Team Build Lab**

In this lab you will create a two releases. One that can deploy the ARM template and either create or update your Azure resources. The second can deploy and update your web sites. 

## Part 1: Create a Service Endpoint

In order to deploy your assets to Azure, you need a **Service Endpoint**.

1.	Access your VSTS Team Project.

1.	From the **Configure** menu ![Configure menu](media/vsts-configure-cog.png "Configure** menu"), select **Services**.

1.	Select **New Service Endpoint**.
 
1.	From the list select **Azure Resource Manager**.

1.	Provide a **Connection name** that makes sense.

1.	Select your **Subscription**.
 
1.	Click **OK**.

## Part 2: Create a Release for Infrastructure

In this lab part, you will create a build that deploys your ARM template to Azure.

1.	Access the **Build & Release** Hub in VSTS.

1.	Select **Releases** and click the **+** button and then **Create release definition** from the drop down.

1.	Select **Empty** from the list of templates and click **Next**.
	
1.	For the **Source (Build definition)**, select **CD Artifact Creation AzureKit - Server Only**.
	
1.	Click **Create**.
	
1.	Click **Add tasks**.
	
1.	From the list of **Deploy** tasks, select **Azure Resource Group Deployment** and clikc **Add**.
	
1.	Click **Close**.
	
1.	For **Azure Subscription**, select the **Service Endpoint** you created earlier.
	
1.	For the **Resource Group**, select your **Resource Group** you used earlier in the labs. If you didn't do the Azure labs, you can type a name here.
	
1.	Select the correct **Location**.
	
1.	In the **Template** field, enter **$(System.DefaultWorkingDirectory)/Build Packages/ARMTemplates/AzureKit.json**.
	
1.	In the **Template Parameters** field enter, **$(System.DefaultWorkingDirectory)/Build Packages/ARMTemplates/AzureKit.parameters.json**.
	
1.	In **Override Template Parameters**, enter the following **-AppPlanSKU Standard -NamePrefix %template% -SqlAdministratorLoginPassword (ConvertTo-SecureString -String '%yourpassword%' -AsPlainText -Force)**. Replace %template% with your prefix and %yourpassword% with your password.
	
1.	Change the **Deployment Mode** to **Complete**.
	
1.	Change the **Environment1** label to **SharedDev**.
	
1.	Change the **Definition** name to **Infrastructure Deployment--AzureKit - Server Only**.

1.	Click **Save**.

1.	In the dialog, enter **Initial deployment for SharedDev** and click **OK**.

1.	Click the **+ Release** button and choose **Create Release** from the drop down menu.

1.	Watch the release run and verify the deployment.

## Part 3: Create a Release for Web sites

This part requires Part 1 to be completed. Part 2 is optional if you've manually created a deployment.

1.	Access the **Build & Release** Hub in VSTS.

1.	Select **Releases** and click the **+** button and then **Create release definition** from the drop down.

1.	Select **Azure App Service Deployment with Slot (PREVIEW)** from the list of templates and click **Next**.
	
1.	For the **Source (Build definition)**, select **CD Artifact Creation AzureKit - Server Only**.
	
1.	Click **Create**.

1.	Select the **Deploy Azure App Service to Slot**.
	
1.	For **Azure Subscription**, select the **Service Endpoint** you created earlier.

1.	In the **App Service name**, select your **website** App Service.

1.	For the **Resource Group**, select your **Resource Group** you used earlier.
	
1.	For the **Slot** select **staging**.
	
1.	For the **Package or Folder** enter **$(System.DefaultWorkingDirectory)/Build Packages/drop/AzureKit.zip**.

1.	Select the **Manage Azure App Service - Slot Swap** task.

1.	For **Azure Subscription**, select the **Service Endpoint** you created earlier.

1.	In the **App Service name**, select your **website** App Service.

1.	For the **Resource Group**, select your **Resource Group** you used earlier.
	
1.	For the **Source Slot** select **staging**.

1.	Change the **Environment1** label to **SharedDev**.
	
1.	Change the **Definition** name to **Site Deployment--AzureKit - Server Only**.

1.	Click **Save**.

1.	In the dialog, enter **Initial deployment of sites for SharedDev** and click **OK**.

1.	At this point you've only done the main web site, you need to now configure the API site and the Management site. Click **Add tasks**.

1.	Find, select, and add **Azure App Service Deploy**.

1.	Find, select, and add **Azure App Service Manage (PREVIEW)**.

1.	Repeat the last two steps.

1.	Select *first* newly added **Azure App Service Deploy** task.
	
1.	For **Azure Subscription**, select the **Service Endpoint** you created earlier.

1.	In the **App Service name**, select your **apisite** App Service.

1.	Check **Deploy to slot**.

1.	For the **Resource Group**, select your **Resource Group** you used earlier.
	
1.	For the **Slot** select **staging**.
	
1.	For the **Package or Folder** enter **$(System.DefaultWorkingDirectory)/CD Artifact Creation AzureKit - Server Only/drop/AzureKit.Api.zip**.

1.	Select *first* newly added **Swap Slots** task.

1.	For **Azure Subscription**, select the **Service Endpoint** you created earlier.

1.	In the **App Service name**, select your **apisite** App Service.

1.	For the **Resource Group**, select your **Resource Group** you used earlier.
	
1.	For the **Source Slot** select **staging**.

1.	Select *second* newly added **Azure App Service Deploy** task.
	
1.	For **Azure Subscription**, select the **Service Endpoint** you created earlier.

1.	In the **App Service name**, select your **managementsite** App Service.

1.	Check **Deploy to slot**.

1.	For the **Resource Group**, select your **Resource Group** you used earlier.
	
1.	For the **Slot** select **staging**.
	
1.	For the **Package or Folder** enter **$(System.DefaultWorkingDirectory)/CD Artifact Creation AzureKit - Server Only/drop/AzureKit.Management.zip**.

1.	Select *second* newly added **Swap Slots** task.

1.	For **Azure Subscription**, select the **Service Endpoint** you created earlier.

1.	In the **App Service name**, select your **managementsite** App Service.

1.	For the **Resource Group**, select your **Resource Group** you used earlier.
	
1.	For the **Source Slot** select **staging**.

1.	Click **Save**.

1.	In the dialog, enter **Updated deployment of sites for SharedDev** and click **OK**.

1.	Click the **+ Release** button and choose **Create Release** from the drop down menu.

1.	Watch the release run and verify the deployment.
