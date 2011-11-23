using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace AridiaEditor
{
    public struct Error
    {
        public ErrorType Type { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }

    }

    public struct Output
    {
        public static void AddToOutput(String message)
        {
            if (MainWindow.outputTextBlock != null)
                MainWindow.outputTextBlock.Dispatcher.Invoke(
                      System.Windows.Threading.DispatcherPriority.Normal,
                      new Action(
                        delegate()
                        {
                            MainWindow.outputTextBlock.Text+= message + "\n";
                        }
                    ));
        }

        public static void AddToError(Error error)
        {
            if (MainWindow.errors != null)
                MainWindow.errors.Add(error);
        }
    }

    public enum ErrorType
    {
        FATAL,
        INFO,
        DEBUG,
        LIGHT
    }
}
