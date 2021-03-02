﻿// RequestR
// Copyright (C) 2021 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace DustInTheWind.RequestR.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Setup request bus
            RequestBus requestBus = new RequestBus();
            requestBus.RegisterUseCase<PresentProductsUseCase>();

            // Send request
            PresentProductsRequest request = new PresentProductsRequest();
            requestBus.Process(request);
            List<Product> products = requestBus.Process<PresentProductsRequest, List<Product>>(request);

            // Display response
            foreach (Product product in products)
            {
                Console.WriteLine();
                Console.WriteLine("Product: " + product.Name);
                Console.WriteLine("Price: " + product.Price);
                Console.WriteLine("Quantity: " + product.Quantity);
            }
        }
    }
}