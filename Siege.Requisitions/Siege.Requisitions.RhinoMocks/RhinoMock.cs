﻿/*   Copyright 2009 - 2010 Marcus Bratton

     Licensed under the Apache License, Version 2.0 (the "License");
     you may not use this file except in compliance with the License.
     You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

     Unless required by applicable law or agreed to in writing, software
     distributed under the License is distributed on an "AS IS" BASIS,
     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     See the License for the specific language governing permissions and
     limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rhino.Mocks;
using Siege.Requisitions.Extensions.AutoMocking;
using Siege.Requisitions.Registrations;

namespace Siege.Requisitions.RhinoMocks
{
    public abstract class RhinoMock<T>
    {
        public static List<IRegistration> Using(MockRepository repository)
        {
            var registrations = new List<IRegistration>();

            Register(typeof (T), typeof (T), repository, registrations);

            return registrations;
        }

        private static object Register(Type baseType, Type to, MockRepository repository, ICollection<IRegistration> registrations)
        {
            if (to.IsInterface)
            {
                object mock = repository.DynamicMock(to);
                registrations.Add(new AutoMockRegistration(to, mock));

                return mock;
            }

            if (to.IsClass)
            {
                ConstructorInfo[] constructors = to.GetConstructors();
                int args = constructors.Max(constructor => constructor.GetParameters().Count());
                ConstructorInfo candidate = constructors.Where(constructor => constructor.GetParameters().Count() == args).FirstOrDefault();
                var parameters = new List<object>();

                foreach (ParameterInfo dependency in candidate.GetParameters())
                {
                    parameters.Add(Register(baseType, dependency.ParameterType, repository, registrations));
                }

                if (baseType != to)
                {
                    object stub = repository.Stub(to, parameters.ToArray());
                    registrations.Add(new AutoMockRegistration(to, stub));

                    return stub;
                }

                object instance = candidate.Invoke(parameters.ToArray());

                registrations.Add(new AutoMockRegistration(to, instance));

                return instance;
            }

            return null;
        }
    }
}