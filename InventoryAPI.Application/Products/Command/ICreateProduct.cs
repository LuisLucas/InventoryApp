using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAPI.Application.Products.Command {
    public interface ICreateProduct {
        Task<int> Handle(CreateProductCommand request);
    }
}
