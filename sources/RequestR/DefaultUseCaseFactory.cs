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

namespace DustInTheWind.RequestR
{
    /// <summary>
    /// This default implementation uses the <see cref="Activator"/> class to create use case instances.
    /// As a result it can create only use cases that provide a parameterless constructor.
    /// </summary>
    public class DefaultUseCaseFactory : UseCaseFactoryBase
    {
        protected override object CreateInternal(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
