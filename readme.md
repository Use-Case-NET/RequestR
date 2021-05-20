# RequestR - Getting Started

## 1) Create the Request, Response and Use Case classes

The request and the response can be any C# classes.

```csharp
internal class PresentProductsRequest
{
}

internal class PresentProductsResponse
{
}
```

The use case must implement the `IUseCase` interface.

```csharp
internal class PresentProductsUseCase : IUseCase<PresentProductsRequest, PresentProductsResponse>
{
    public PresentProductsResponse Execute(PresentProductsRequest request, CancellationToken cancellationToken)
    {
        // Return the list of products.
    }
}
```

Note: The response class is optional, it may be missing if the use case has nothing to return.

## 2) Create the Request Bus and register the Use Case

```csharp
RequestBus requestBus = new RequestBus();
requestBus.RegisterUseCase<PresentProductsUseCase>();
```

## 3) Send a new Request

```csharp
PresentProductsRequest request = new PresentProductsRequest();
PresentProductsResponse response = requestBus.Process<PresentProductsRequest, PresentProductsResponse>(request);
```
