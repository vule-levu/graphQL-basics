# graphQL-basics
A simple .Net Webapi application with graphQL
1. install Microsoft's EF InMemory
2. install HotChocolate Aspnetcore
3. Try `dotnet run`
4. If step 3 succeeds, try querying:
```
{
    usersWithPosts(userIds: [1, 2]) {
        user {
            id
            name
        }
        posts {
            title
        }
    }
}
```

