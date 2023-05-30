using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace MetaComponent
{
    public class MetaComponentInfo : GH_AssemblyInfo
    {
        public override string Name => "MetaComponent";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("c836a092-1f28-4c14-9b76-c181e7228fc8");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}