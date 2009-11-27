using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Collections;

namespace VirtualBicycle.Graphics
{
    /// <summary>
    ///  表示顶点声明
    /// </summary>
    public abstract class VertexDeclaration : IDisposable
    {
        #region 字段

        /// <summary>
        ///     List of elements that make up this declaration.
        /// </summary>
        protected FastList<VertexElement> elements = new FastList<VertexElement>();

        #endregion


        protected VertexDeclaration(VertexElement[] elements)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                this.elements.Add(elements[i]);
            }
        }


        #region 方法

        /// <summary>
        ///     Finds a <see cref="VertexElement"/> with the given semantic, and index if there is more than 
        ///     one element with the same semantic. 
        /// </summary>
        /// <param name="semantic">Semantic to search for.</param>
        /// <returns>If the element is not found, this method returns null.</returns>
        public bool FindElementBySemantic(VertexElementUsage semantic, out VertexElement result)
        {
            // call overload with a default of index 0
            return FindElementBySemantic(semantic, 0, out result);
        }

        /// <summary>
        ///     Finds a <see cref="VertexElement"/> with the given semantic, and index if there is more than 
        ///     one element with the same semantic. 
        /// </summary>
        /// <param name="semantic">Semantic to search for.</param>
        /// <param name="index">Index of item to looks for using the supplied semantic (applicable to tex coords and colors).</param>
        /// <returns>If the element is not found, this method returns null.</returns>
        public virtual bool FindElementBySemantic(VertexElementUsage semantic, int index, out VertexElement result)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                VertexElement element = elements[i];

                // do they match?
                if (element.Semantic == semantic && element.Index == index)
                {
                    result = element;
                    return true;
                }
            }

            // not found
            result = default(VertexElement);
            return false;
        }


        /// <summary>
        ///		Gets the <see cref="VertexElement"/> at the specified index.
        /// </summary>
        /// <param name="index">Index of the element to retrieve.</param>
        /// <returns>Element at the requested index.</returns>
        public VertexElement GetElement(int index)
        {
            //Debug.Assert(index < elements.Count && index >= 0, "Element index out of bounds.");

            return elements[index];
        }

        /// <summary>
        ///     Gets the vertex size defined by this declaration for a given source.
        /// </summary>
        /// <param name="source">The buffer binding index for which to get the vertex size.</param>
        public virtual int GetVertexSize()
        {
            int size = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                //VertexElement element = elements[i];

                //// do they match?
                //if (element.Source == source)
                size += elements[i].Size;
            }

            // return the size
            return size;
        }


        /// <summary>
        ///     Tests equality of 2 <see cref="VertexElement"/> objects.
        /// </summary>
        /// <param name="left">A <see cref="VertexElement"/></param>
        /// <param name="right">A <see cref="VertexElement"/></param>
        /// <returns>true if equal, false otherwise.</returns>
        public static bool operator ==(VertexDeclaration left, VertexDeclaration right)
        {
            // if element lists are different sizes, they can't be equal
            if (left.elements.Count != right.elements.Count)
                return false;

            for (int i = 0; i < right.elements.Count; i++)
            {
                VertexElement a = left.elements[i];
                VertexElement b = right.elements[i];

                // if they are not equal, this declaration differs
                if (!(a == b))
                    return false;
            }

            // if we got thise far, they are equal
            return true;
        }

        /// <summary>
        ///     Tests in-equality of 2 <see cref="VertexElement"/> objects.
        /// </summary>
        /// <param name="left">A <see cref="VertexElement"/></param>
        /// <param name="right">A <see cref="VertexElement"/></param>
        /// <returns>true if not equal, false otherwise.</returns>
        public static bool operator !=(VertexDeclaration left, VertexDeclaration right)
        {
            return !(left == right);
        }

        #endregion

        #region 属性

        /// <summary>
        ///     Gets the number of elements in the declaration.
        /// </summary>
        public int ElementCount
        {
            get
            {
                return elements.Count;
            }
        }

        #endregion

        #region Object overloads

        /// <summary>
        ///    Override to determine equality between 2 VertexDeclaration objects,
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            VertexDeclaration decl = obj as VertexDeclaration;

            return (decl == this);
        }

        /// <summary>
        ///    Override GetHashCode.
        /// </summary>
        /// <remarks>
        ///    Done mainly to quash warnings, no real need for it.
        /// </remarks>
        /// <returns></returns>
        // TODO: Does this need to be implemented, dont think we are stuffing these into hashtables.
        public override int GetHashCode()
        {
            return 0;
        }

        #endregion Object overloads

        //#region ICloneable Members

        ///// <summary>
        /////     Clonses this declaration, including a copy of all <see cref="VertexElement"/> objects this declaration holds.
        ///// </summary>
        ///// <returns></returns>
        //public object Clone()
        //{
        //    VertexDeclaration clone = VertexDeclarationManager.Instance.CreateVertexDeclaration();

        //    for (int i = 0; i < elements.Count; i++)
        //    {
        //        VertexElement element = (VertexElement)elements[i];
        //        clone.AddElement(element.Offset, element.Type, element.Semantic, element.Index);
        //    }

        //    return clone;
        //}

        //#endregion

        #region IDisposable 成员

        public abstract void Dispose(bool disposing);

        public bool Disposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);
                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        ~VertexDeclaration()        
        {
            if (!Disposed)
            {
                Dispose(false);
                Disposed = true;
            }
        }

        #endregion
    }

}
