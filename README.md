# Social Media Analysis

This project was used to collect social media posts from Twitter, Facebook, and Google Plus from a list of keywords. It also attempted to classify the posts as positive or negative, using a naïve Bayes classifier.

Unfortunately there is no way to really run this service now because all the data was handled in Azure, and is now offline. Also the facebook and twitter APIs have changed significantly since this was written.

## Highlights

1. Controllers/SocialMonitorAPIController.cs
    * The primary API used to control the service.
2. DatabaseAccessLayer.cs
    * The data layer used to read and write all data
3. ChartManager.cs
    * Class used to organize the aggregated data into useable charts
4. Views/Charts.cshtml
    * Charts rendered using Google Charts
5. Schema/DatabaseSchemaUtility.cs
    * Utility to automatically generate all necessary tables and stored procedures for new instances of the service
6. DataCollector.cs
    * The class that ran constantly to collect the social media data.
7. Sources/*
    * These are the classes used to interface with various social media platforms. This area required the most maintenance because Facebook and Twitter were constantly changing.
