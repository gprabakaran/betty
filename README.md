# Betty
Welcome to the repository of Betty, your friendly check-in counter bot.
This bot is developed with Bot Framework 4 in C#. 

## Getting started
This bot requires a number of resources, please make sure you have the
following resources in your Azure subcription:

 * Language Understanding Service
 * QnA Maker 

Additionally, make sure that you've installed the bot tooling using npm.

```
npm i -g msbot luis ludown qnamaker
```

### Preparing the bot file
The software uses a bot file to configure the endpoints and services
used by the bot. Please make sure you create a new bot file using the 
following command:

```
msbot init
```

Give the bot the name `Betty` and make sure you enable encryption using secrets.
Save the secret in `appsettings.json` under the setting `BotConfigurationSecret`.

Make a new endpoint in the configuration file using the following command:

```
msbot connect endpoint -n Development -e "http://localhost:20168/api/messages" -b Betty.bot --secret <secret>
```

Replace the `<secret>` with the secret you saved when you created the bot file.

### Preparing the LUIS model
To load the intents of the bot into LUIS, follow these steps:

 * `ludown parse toluis --in Betty.lu --out_folder .\data\models -n <model> -i 0.1`
 * `luis import version --authoringKey <key> --in .\models\Betty.json`

Replace `<model>` with the name of the LUIS model you created on Azure.
Replace `<key>` with the authoring key for your LUIS model on Azure.

Connect the bot to the LUIS service using the following command:

```
msbot connect luis -n Development-luis -a <appId> -r <region> --subscriptionKey <subscriptionKey> --version "0.1" --authoringKey <authoringKey> --secret <secret> -b Betty.bot
```

Replace the following parameters in the command:

<table>
  <thead>
    <tr>
      <th>Parameter</th>
      <th>Description</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>appId</td>
      <td>The Application ID of the LUIS model. You can find this on the settings page of the LUIS model on the LUIS management portal.</td>
    </tr>
    <tr>
      <td>region</td>
      <td>The region where you deployed your LUIS service.</td>
    </tr>
        <tr>
      <td>subscriptionKey</td>
      <td>The key used to authorize access to the LUIS model. You can find this on the Azure Portal when you open the *Keys* page of the LUIS service.</td>
    </tr>
        <tr>
      <td>authoringKey</td>
      <td>The authoring key. This can be found on the settings page of your LUIS model on the LUIS management portal.</td>
    </tr>
    <tr>
      <td>secret</td>
      <td>The secret used to create the bot file.</td>
    </tr>
  </tbody>
</table>

### Preparing the QnA Maker model
Create a standard QnA Maker knowledge base on Azure.
Execute the following command to connect the bot to the QNA Maker resource:

```
msbot connect Development-qna --hostname <url> -n Development-qna -k <knowledgeBaseId> k --subscriptionKey <subscriptionKey> --secret "<secret>" -b Betty.bot
```

<table>
  <thead>
    <tr>
      <th>Parameter</th>
      <th>Description</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>knowledgeBaseId</td>
      <td>The ID of the knowledgebase. You can find this on the QnAMaker portal.</td>
    </tr>
    <tr>
      <td>subscriptionKey</td>
      <td>The subscription key for the QnAMaker service. You can find this on the *Keys* page when you've selected the QnAMaker resource on the Azure Portal.
    </tr>
    <tr>
      <td>secret</td>
      <td>The secret used to create the bot file earlier.</td>
    </tr>
  </tbody>
</table>

## Running the bot
You can run the bot using the command `dotnet run`.
Or run the code from Visual Studio Code with <kbd>F5</bkd>

## Issues or comments?
Please file an issue if you have any questions or problems.
I'll try to get back to you as soon as I can.