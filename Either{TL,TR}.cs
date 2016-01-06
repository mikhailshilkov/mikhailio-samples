namespace MikhailIo
{
    using System;

    public class Either<TL, TR>
    {
        private readonly TL left;
        private readonly TR right;
        private readonly bool isLeft;

        public Either(TL left)
        {
            this.left = left;
            this.isLeft = true;
        }

        public Either(TR right)
        {
            this.right = right;
            this.isLeft = false;
        }

        public T Match<T>(Func<TL, T> leftFunc, Func<TR, T> rightFunc)
            =>  this.isLeft ? leftFunc(this.left) : rightFunc(this.right);

        public static implicit operator Either<TL, TR>(TL left) => new Either<TL, TR>(left);

        public static implicit operator Either<TL, TR>(TR right) => new Either<TL, TR>(right);
    }
}
