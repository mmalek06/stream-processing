﻿using System.Text;
using StreamProcessing.Server.Resources;

Console.WriteLine(@"Running data sender - this will simulate the process of sending data belonging to one wind turbine.");

var lines = Encoding
    .UTF8
    .GetString((byte[])Resource.ResourceManager.GetObject("T1", Resource.Culture)!)
    .Split(Environment.NewLine);
var counter = 0;
const int InformEveryNLines = 1000;

foreach (var line in lines)
{
    
    
    counter++;
    
    if (counter % InformEveryNLines == 0)
        Console.WriteLine(@$"Imported {counter} lines so far.");
}

Console.WriteLine(@"Data sender finished, press any key to exit...");
Console.ReadKey();