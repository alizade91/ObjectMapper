using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectMapper
{
    class Program
    {
        static void Main(string[] args)
        {
            DomainObject domainObject = new DomainObject { Name = "Unknown", Address = "Usxod",Age=26 };
            var mapper = new Mapper<DomainObject, ViewModel>();
            var viewModel = mapper.CreateMappedObject(domainObject);
            Console.WriteLine(viewModel.Name);
            Console.WriteLine(viewModel.Address);
        }
    }

}
