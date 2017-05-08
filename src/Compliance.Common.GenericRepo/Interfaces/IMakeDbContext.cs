﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.Common.GenericRepo.Interfaces
{
    public interface IMakeDbContext<TContext>
    {
        DbContext GetContext();
    }
}