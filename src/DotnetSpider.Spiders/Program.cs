﻿using System;

namespace DotnetSpider.Spiders
{
	public class Program
	{
		static void Main()
		{
			var startup = new MyStartup();
			startup.Execute();
			Console.WriteLine("Bye");
		}
	}
}