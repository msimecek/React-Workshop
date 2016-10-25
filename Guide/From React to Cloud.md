# From React to Azure

> Reactive Conf - Workshop

During this workshop you will learn how to connect your React application to Azure and make it more intelligent using Microsoft Cognitive Services.

## Prepare Azure Account

To use Azure you need a Microsoft Account. If you don't have one, [register on the web](https://signup.live.com).

...

Once you have your account and subscription ready, sign in to the [Azure portal](https://portal.azure.com).

## Create Resources

Using the **+ New** button you create all the Azure Resources that are available. You can either pick from categories, or search directly. The search will lead you to Azure Marketplace which contains different apps and services from Microsoft and partners.

We will start by creating a **Resource Group**:

1. Click **+ New**

2. Search for *Resource Group*

    ![01-create-rg](Images/01-create-rg.png)

3. Pick the first result - **Resource group** from Microsoft

    ![02-resource-group](Images/02-resource-group.png)

4. Click **Create**

5. Pick any name for the group, check **Pin to dashboard** and click **Create**

6. At dashboard click on your newly created Resource Group

Now you have the basic logical container for your app ready. Let's continue by adding some resources to it. We are going to need Cognitive Services and Functions.

### Cognitive Services API

1. In the **Overview** section click **Add** at the top

    ![03-add-to-rg](Images/03-add-to-rg.png)

2. Search for *cognitive services*

3. Select **Cognitive Services APIs (preview)** from Microsoft

    ![04-cognitive-services](Images/04-cognitive-services.png)

4. Click **Create**

5. Enter any account name

6. Select **API Type** as **Emotion API (preview)** 

    ![05-emotion-api](Images/05-emotion-api.png)

7. In **Pricing tier** select **Free** 

8. Make sure the Resource group section has **Use existing** selected and that it shows the name of your recently created resource group

9. In **Legal terms** click **Review legal terms** and **I Agree**

10. Finally, click **Create** 

After the API is created it will be shown back in your Resource Group (you may have to open it again or click Refresh). Open its blade by clicking it and go to the **Keys** section. From there copy the value of **KEY 1**:

 ![06-emotion-keys](Images/06-emotion-keys.png)

Save it somewhere (text file for instance) as we will need it later on.

### Function App

Last thing to create will be a Function App. The process is quite similar to creating a Cognitive Services account. Just go back to the Resource Group, click Add, search for Function App, choose Function App, click Create and then:

1. Enter **App name** which needs to be **globally unique** (as it's going to be a part of the URL)
2. Check again if the Resource group is correct
3. Keep **App Service plan** as **Dynamic**
4. Change **Location** to **North Europe** (to make it closer to us)
5. Keep Memory Allocation **128 MB**
6. Keep Storage Account as is - it will create a new Storage which is needed for our app
7. Click **Create**

At this stage your Resource Group should contain everything that is needed for this workshop:

 ![07-created](Images/07-created.png)

## Create the API

Now let's do something more interesting, let's code! We'll create the API, set it to wait for HTTP request containing an image, process it through Cognitive Services API and return emotion results as JSON.

1. Open the Function App you created in previous step

2. Click **+ New Function**

    ![08-new-function](Images/08-new-function.png)

3. Select the **HttpTrigger - C#** template

    ![09-http-trigger](Images/09-http-trigger.png)

4. Scroll down and name give your function a meaningful **name** (such as FaceProcessor...)

5. Set **Authorization level** to **Anonymous**, since we'll allow anyone to call the API

   > In production applications you should set the autorization level and require an API key to be present with the call.

6. Click **Create**

The app will get created and prepopulated with sample C# code. You can notice that it will get an `HttpRequestMessage` object which should contain the image we want to process.

```csharp
using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");
...
```

Now go to [this code file](https://github.com/msimecek/React-Workshop/blob/master/EmotionDetector.csx), copy the code and replace everything in the **Code** window. Then click **Save** and see in the Logs section if it compiled without errors.

The API will now run every time someone invokes it via HTTP request. But it's not ready yet! We are missing configuration.

## Configure the API

You need to set up two things: the Emotion API key and CORS.

If you look closely at the code we just pasted, you will notice that we're setting a header called `Ocp-Apim-Subscription-Key`. It's value comes from an environmental variable called *EmotionApiKey*.

    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("EmotionApiKey"));
Let's set it up.

1. Leave the Develop section of your Function App and click **Function app settings** at the bottom.

    ![10-function-app-settings](Images/10-function-app-settings.png)

2. Scroll down and select **Configure app settings** button next to Application settings

3. On the newly opened blade scroll down to the **App settings** section

4. Add new key called **EmotionApiKey** with value of KEY 1 for your Emotion API (Remember? We copied it earlier and saved it to a text file.)

    ![11-emotion-api-key](Images\11-emotion-api-key.png)

5. Click **Save** at the top of the blade

Calls to Cognitive Services will now work. But we need to set up one more thing to make our API accessible from JavaScript - set up [CORS](https://en.wikipedia.org/wiki/Cross-origin_resource_sharing) headers.

1. Close the Application settings blade and go back to Function App settings

2. Click the **Configure CORS** button

3. Delete all prepopulated origins

    ![12-cors-delete](Images\12-cors-delete.png)

4. Add new origin: *****

    ![13-cors-star](Images\13-cors-star.png)

5. Click **Save** 

The API is now properly configured and ready to be called from React.