﻿using System;
using NUnit.Framework;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Xaml;

[assembly: XamlResourceId("Tizen.NUI.Devel.Tests.testcase.public.Xaml.TotalSample.FactoryMethodMissingCtor.xaml",
    "testcase.public.Xaml.TotalSample.FactoryMethodMissingCtor.xaml",
    typeof(global::Tizen.NUI.Devel.Tests.FactoryMethodMissingCtor))]

namespace Tizen.NUI.Devel.Tests
{
    [XamlFilePath("testcase\\public\\Xaml\\TotalSample\\FactoryMethodMissingCtor.xaml")]
    [XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class FactoryMethodMissingCtor : MockView
	{
        public FactoryMethodMissingCtor()
        {
            global::Tizen.NUI.Xaml.Extensions.LoadFromXaml(this, typeof(FactoryMethodMissingCtor));
        }
	}

    [TestFixture]
    public class FactoryMethodMissingCtorTests
    {
        [SetUp] public void Setup() { }

        [TearDown] public void TearDown() { }

        [Test]
        [Category("P1")]
        [Description("Extensions LoadFromXaml.")]
        [Property("SPEC", "Tizen.NUI.Xaml.Extensions.LoadFromXaml M")]
        [Property("SPEC_URL", "-")]
        [Property("CRITERIA", "MR")]
        public void FactoryMethodMissingCtorThrow()
        {
            Assert.Throws<MissingMethodException>(() => new FactoryMethodMissingCtor());
        }
    }
}