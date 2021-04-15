﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.Helpers
{
    public class FrameworkVocabularyHelper
    {
        public static string VocabularySingular(string? vocab)
        {
            if (vocab == null)
            {
                return "Capability";
            }
            else
            {
                return vocab;
            }
        }
        public static string VocabularyPlural(string? vocab)
        {
            if (vocab == null)
            {
                return "Capabilities";
            }
            else
            {
                if (vocab.EndsWith("y"))
                {
                    return vocab.Substring(0, vocab.Length - 1) + "ies";
                }
                else
                {
                    return vocab + "s";
                }
            }
        }
    }
}
