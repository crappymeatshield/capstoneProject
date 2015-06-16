//Reads Xml file
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using XmlContentSampleShared;

// TODO: replace these with the processor input and output types.
using TInput = System.String;
using TOutput = System.String;

namespace XmlContentSampleExtension
{
    [ContentTypeWriter]
    public class ContentTypeWriter :ContentTypeWriter<Sprite>
    {
        protected override void Write(ContentWriter output, Sprite value)
        {
            //output.Write(value.Rows);
            //output.Write(value.Cols);
            output.Write(value.Floorsquare);
            output.Write(value.Boxnum);
            //output.Write(value.TextureAsset);
        }
        public override string GetRuntimeReader(TargetPlatform targetplatform)
        {
            return typeof(XmlContentSampleShared.Sprite.SpriteContentReader).AssemblyQualifiedName;
        }
    }
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
}