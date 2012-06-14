using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lokad_iddd_sample
{
    public static class Program
    {
        public static void Main()
        {
            var store = CreateFileStoreForTesting();
            var events = new EventStore(store);
            //var store = new FileEventStore(new SampleStrategy(), "temp");
            

            var server = new Server();
            server.Handlers.Add(new CustomerApplicationService(events));

            server.Dispatch(new CreateCustomer { Id = new CustomerId(12), Name = "Lokad"});
            server.Dispatch(new RenameCustomer { Id = new CustomerId(12), NewName = "Lokad SAS"});

            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);
        }

        static IAppendOnlyStore CreateSqlStore()
        {
            var conn = "Data Source=.\\SQLExpress;Initial Catalog=lokadsalescast_samples;Integrated Security=true";
            var store = new SqlAppendOnlyStore(conn);
            store.Initialize();
            return store;
        }
        static IAppendOnlyStore CreateFileStoreForTesting()
        {
            {
                // reset the store, since we are testing
            }
            var combine = Path.Combine(Directory.GetCurrentDirectory(), "store");
            if (Directory.Exists(combine))
            {
                Console.WriteLine("Wiping even store for demo purposes");
                Directory.Delete(combine, true);
            }
            var store = new FileAppendOnlyStore(combine);
            store.Initialize();
            return store;
        }

        public sealed class Server
        {
            public void Dispatch(ICommand cmd)
            {
                Console.WriteLine(cmd);
                foreach (var handler in Handlers)
                {
                    ((dynamic) handler).When((dynamic) cmd);
                }
            }

            public readonly IList<object> Handlers = new List<object>(); 
        }
    }
}