using AltV.Net;
using AltV.Net.Data;
using Backend.Controllers.Federal.Interface;
using Backend.Services.FederalLicense.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.Federal
{
    public class FederalController : IFederalController
    {
        private readonly IFederalLicenseService _federalLicenseService;

        private readonly Position _licensePosition = new (2437.543f, -389.86813f, 93.09009f);

        public FederalController(IFederalLicenseService federalLicenseService)
        {
            _federalLicenseService = federalLicenseService;

            LoadFederalShapes();
        }

        public void LoadFederalShapes()
        {
            var shape = (ClShape)Alt.CreateColShapeSphere(_licensePosition, 1f);
            shape.Id = 1;
            shape.Dimension = 0;
            shape.Size = 1f;
            shape.ShapeType = ColshapeType.FEDERAL_LICENSE;
        }
    }
}
