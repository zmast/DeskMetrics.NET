About this fork
===============

This fork is the result of a massive set of changes.
Here are the biggest changes:

 - Resolved bug that sent twice the same data on Stop()
 - Fixed cache management when using SendAsync
 - Cleaned up the messy session-id handling between classes
 - Simplified cache and user guid management
 - Check for corrupted cache content
 - Removed some class circular dependencies
 - Documented the public method/properties
 - Made internal thoose classes not supposed to be used outside the library
 - Changed variable names to follow standard C# naming conventions
 - Took advantage of C# 3.0 auto properies


Please note that this is NOT the official library and while it should be completely compatible it was not tested enterily.

For the official library see [deskmetrics/DeskMetrics.NET](https://github.com/deskmetrics/DeskMetrics.NET)



DeskMetrics.NET -- .NET assembly for DeskMetrics Analytics
===========================================================

DeskMetrics is a Google Analytics-like service for non-html based apps. You can read more at [DeskMetrics website](http://deskmetrics.com/)

This assembly aims to provide a simple interface to integrate your .NET-based app with DeskMetrics service.


How to use:
------------

There are two very important functions to integrate your application to our service:

 - `DeskMetricsStart`
 - `DeskMetricsStop`

Calling these two methods is mandatory inside your app, because they're responsible for session generating, system information tracking and for reporting the captured events to DeskMetrics server.

Here is an example:

    using DeskMetrics;

    namespace MyApplication
    {
        public partial class Form1 : Form
        {
            DeskMetrics.Watcher DeskMetrics = new DeskMetrics.Watcher();

            public Form1()
            {
                DeskMetrics.Start("YOUR APPLICATION ID", "1.0");
                InitializeComponent();
            }

            private void Form1_FormClosing(object sender, FormClosingEventArgs e)
            {
                DeskMetrics.Stop();
            }
        }
    }

You can read more in [DeskMetrics support page](http://support.deskmetrics.com/kb/getting-started/integrating-the-component)

License
--------

This code is provided under the DeskMetrics Modified BSD License  
A copy of this license has been distributed in a file called      
LICENSE with this source code.                                    
