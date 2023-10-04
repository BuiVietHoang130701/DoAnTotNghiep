using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep.Data
{
    public class DoAnTotNghiepContext : IdentityDbContext<User>
    {
        public DoAnTotNghiepContext(DbContextOptions<DoAnTotNghiepContext> options)
            : base(options)
        {
        }

    }
}
