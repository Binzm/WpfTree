
namespace TreeTest.ProductAndCustomer
{
    public class Producer
    {
        private readonly AsyncStack _asyncStack=null;

        public Producer(AsyncStack asyncStack)
        {
            this._asyncStack = asyncStack;
        }

        public void RunProduction(bool handler)
        {
                _asyncStack.Push(handler);
        }

    }
}
