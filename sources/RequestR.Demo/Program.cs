// RequestR
// Copyright (C) 2021-2025 Dust in the Wind
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

namespace DustInTheWind.RequestR.Demo;

internal static class Program
{
    private static void Main(string[] args)
    {
        // Setup request bus
        RequestBus requestBus = new();
        requestBus.RegisterUseCase<PresentProductsUseCase>();

        // Send request
        PresentProductsRequest request = new();
        requestBus.Send(request);
        List<Product> products = requestBus.Send<PresentProductsRequest, List<Product>>(request);

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