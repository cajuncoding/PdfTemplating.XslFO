/*
Copyright 2012 Brandon Bernard

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
using System.Media;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Linq.CustomExtensions;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.CustomExtensions;
using System.Reflection;
using System.Reflection.CustomExtensions;
using System.CustomExtensions;

namespace TE.Library
{
    public class EmbeddedResource
    {
        public static Stream LoadEmbeddedResourceDataAsStream(String resourceNameRegexPattern)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            var enumerableStreams = assembly.GetManifestResourceStreamsByRegex(resourceNameRegexPattern);
            return enumerableStreams.FirstOrDefault();
        }


        public static String LoadEmbeddedResourceDataAsString(String resourceNameRegexPattern)
        {
            using (Stream resourceStream = LoadEmbeddedResourceDataAsStream(resourceNameRegexPattern))
            {
                if (resourceStream != null)
                {
                    return resourceStream.ReadString();
                }
            }
            return null;
        }

        public static XDocument LoadEmbeddedTestDataAsXml(String resourceNameRegexPattern)
        {
            using (Stream resourceStream = LoadEmbeddedResourceDataAsStream(resourceNameRegexPattern))
            {
                if (resourceStream != null)
                {
                    return resourceStream.ToXDocument();
                }
            }
            return null;
        }

        public static SoundPlayer LoadEmbeddedResourceAsSound(String resourceNameRegexPattern)
        {
            //NOTE:  We Do NOT wrap the Stream in a Using Block because it Cannot Be closed before the SoundPlayer
            //       accesses it.  We assume the caller knows that SoundPlayer itself is IDisposable will handle
            //       the clean up for it and it's internal stream.
            Stream soundStream = LoadEmbeddedResourceDataAsStream(resourceNameRegexPattern);
            return new SoundPlayer(soundStream);
        }
    }
}
