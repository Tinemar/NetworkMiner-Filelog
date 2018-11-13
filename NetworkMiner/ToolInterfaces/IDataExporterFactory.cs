﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkMiner.ToolInterfaces {
    public interface IDataExporterFactory {
        IDataExporter CreateDataExporter(string filename, bool preserveNewlineCharacters);
    }
}
