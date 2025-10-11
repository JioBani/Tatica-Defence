using UnityEngine;

namespace Common.TransformChildrenIterator
{
    public static class TransformChildrenIterator
    {
        public struct TransformForwardIterator
        {
            private Transform m_Trans;
            private int m_Index;
            public TransformForwardIterator(Transform aTransform)
            {
                m_Trans = aTransform;
                m_Index = -1;
            }
            public bool MoveNext() => ++m_Index < m_Trans.childCount;
            public Transform Current => m_Trans == null ? null : m_Trans.GetChild(m_Index);
            public TransformForwardIterator GetEnumerator()=> this;

        }
        public struct TransformBackwardIterator
        {
            private Transform m_Trans;
            private int m_Index;
            public TransformBackwardIterator(Transform aTransform)
            {
                m_Trans = aTransform;
                m_Index = m_Trans.childCount;
            }
            public bool MoveNext() => --m_Index >= 0;
            public Transform Current => m_Trans == null ? null : m_Trans.GetChild(m_Index);
            public TransformBackwardIterator GetEnumerator() => this;
        }
    
        public static TransformForwardIterator ChildrenForward(this Transform aTransform)
        {
            return new TransformForwardIterator(aTransform);
        }
    
        public static TransformBackwardIterator ChildrenBackward(this Transform aTransform)
        {
            return new TransformBackwardIterator(aTransform);
        }
    }
}


