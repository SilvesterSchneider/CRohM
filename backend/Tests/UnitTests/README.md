# [Best practices](https://docs.microsoft.com/de-de/dotnet/core/testing/unit-testing-best-practices)

#### Naming your tests

The name of your test should consist of three parts:

- The name of the method being tested.
- The scenario under which it's being tested.
- The expected behavior when the scenario is invoked.

---

#### Construction of a unit test

```
public void MethodWhichWillBeTested_Scenario_ExpectedBehavior()
{
	//Arrange

    //Act

    //Assert
}
```

The comments are just for clean code.<br>
Write the related code below the comments.

---

#### Naming your variables

- sut = subject under test
