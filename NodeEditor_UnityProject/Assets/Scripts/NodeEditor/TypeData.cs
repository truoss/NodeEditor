using UnityEngine;
using System;

namespace NodeSystem
{    
    public class TypeData
    {
        public ITypeDeclaration declaration;
        public Type Type;
        public Color col;

        public TypeData(ITypeDeclaration typeDecl)
        {
            declaration = typeDecl;
            Type = declaration.Type;
            col = declaration.col;
        }
    }

    public interface ITypeDeclaration
    {
        string Name { get; }
        Color col { get; }
        Type Type { get; }
    }

    public class FloatType : ITypeDeclaration
    {
        public string Name { get { return "float"; } }
        public Color col { get { return Color.yellow; } }
        public Type Type { get { return typeof(float); } }
    }

    public class Vector3Type : ITypeDeclaration
    {
        public string Name { get { return "Vector3"; } }
        public Color col { get { return Color.green; } }
        public Type Type { get { return typeof(Vector3); } }
    }
}
