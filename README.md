# InventoryApp
# 🧭 HATEOAS API – Clean Hypermedia in ASP.NET Core

This is a learning project to explore **HATEOAS** (Hypermedia As The Engine Of Application State) in a clean, scalable way using ASP.NET Core Web API.

> HATEOAS adds navigable links to your API responses, making them self-descriptive and decoupled from client knowledge.

---

## 🛠️ Tech Stack

- ASP.NET Core Web API
- REST principles
- HATEOAS design patterns
- (Optional) Swagger / OpenAPI

---

## 🌐 Sample Response

```json
{
  "id": 1,
  "name": "Wireless Mouse",
  "price": 25.99,
  "links": [
    { "rel": "self", "href": "/api/products/1", "method": "GET" },
    { "rel": "update", "href": "/api/products/1", "method": "PUT" },
    { "rel": "delete", "href": "/api/products/1", "method": "DELETE" }
  ]
}
```

---

## 🔧 HATEOAS Implementation

- ✅ Custom `Link` and `Resource<T>` wrappers
- ✅ `LinkBuilder` that generates links per route name
- 🚧 Supports dynamic links per object/resource state
- 🚧 Future: Add role-based filtering and link templates

---

## 📦 Getting Started

```bash
git clone https://github.com/LuisLucas/InventoryApp.git
cd InventoryApp
dotnet run
```

---

## 📌 Goals

- Practice designing APIs with hypermedia controls
- Keep HATEOAS logic separate from controllers
- Prepare reusable components for future projects

---

## 📂 Folder Structure

See the `Hateoas/` folder for link-related abstractions.

---

> Still improving this as I explore more hypermedia API concepts – PRs and suggestions welcome!
