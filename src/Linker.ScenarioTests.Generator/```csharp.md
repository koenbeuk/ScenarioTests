```csharp

var t1 = scenario.Theory((user) => {
    Assert.NotNull(user);
})

foreach (var user in users) {
    t1(user);
}


```


```csharp

scenario.Theory(users, (user) => {
    Assert.NotNull(user);
});

```

```csharp

scenario.Theory(
    () => {
        yield return user1;
        yield return user2;
    },
    , (user) => {
        Assert.NotNull(user);
    }
);


```



```csharp

foraech (var user in users) {

    // Test1
    scenario.Theory(() => {
        Assert.NotNull(user);
    });

    // Test2
    scenario.Theory(() => {
        Assert.Foo(user);
    });    
}


[Theory]
scenario.Play("test1");

[Theory]
scenario.Play("test2");
```