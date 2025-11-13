// RequestR
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

namespace DustInTheWind.RequestR.Demo;

internal class PresentProductsUseCase : IUseCase<PresentProductsRequest, List<Product>>
{
    public Task<List<Product>> Execute(PresentProductsRequest request, CancellationToken cancellationToken)
    {
        List<Product> products =
        [
            new Product
            {
                Name = "Chocolate",
                Price = 10,
                Quantity = 15
            },
            new Product
            {
                Name = "Potato Chips",
                Price = 2,
                Quantity = 7
            },
            new Product
            {
                Name = "Water",
                Price = 5,
                Quantity = 10
            }
        ];

        return Task.FromResult(products);
    }
}