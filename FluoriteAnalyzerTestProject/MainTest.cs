using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Windows.Forms;

namespace FluoriteAnalyzerTestProject
{
    [TestClass]
    public class MainTest
    {
        [TestMethod]
        public void AutoScaleModeTest()
        {
            Type typeMainForm = typeof(FluoriteAnalyzer.Forms.MainForm);
            Assembly mainAssembly = typeMainForm.Assembly;

            Type[] allTypes = mainAssembly.GetTypes();
            foreach (Type type in allTypes)
            {
                if (type.IsSubclassOf(typeof(ContainerControl)))
                {
                    // Make sure that the AutoSacleMode value is false
                    ConstructorInfo constructor = type.GetConstructor(new Type[] { });
                    ContainerControl container = null;

                    if (constructor != null)
                    {
                        container = constructor.Invoke(new object[] { }) as ContainerControl;
                    }
                    // Try ILogProvider. All the analysis panels use this constructor.
                    else
                    {
                        constructor = type.GetConstructor(new Type[] { typeof(FluoriteAnalyzer.Common.ILogProvider) });
                        if (constructor != null)
                            container = constructor.Invoke(new object[] { null }) as ContainerControl;
                    }

                    Assert.IsNotNull(container);

                    if (container != null)
                    {
                        Assert.AreEqual(AutoScaleMode.None, container.AutoScaleMode, "Check the \"AutoScaleMode\" property value of \"" + type.FullName + "\"");
                    }
                }
            }
        }
    }
}
