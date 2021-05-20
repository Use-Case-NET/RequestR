# RequestR - Getting Started

## 1) Create the Request and Request Handler classes

The request can be any C# class.

```csharp
internal class PresentProductsRequest
{
}
```

The request handler must implement the `IRequestHandler` interface.

```csharp
internal class PresentProductsUseCase : IUseCase<PresentProductsRequest, List<Product>>
{
    public Task<List<Product>> Execute(PresentProductsRequest request, CancellationToken cancellationToken)
    {
        // Return the list of products.
    }
}
```

## 2) Create the Request Bus and register the Request Handler

```csharp
RequestBus requestBus = new RequestBus();
requestBus.RegisterHandler<PresentProductsRequestHandler>();
```

## 3) Send a new Request

```csharp
PresentProductsRequest request = new PresentProductsRequest();
List<Product> products = requestBus.Send<PresentProductsRequest, List<Product>>(request);
```
