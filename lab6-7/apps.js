const products = [
  { id: 1, name: 'Black Hoodie',    price: 59,  category: 'hoodie',  inStock: true  },
  { id: 2, name: 'White T-Shirt',   price: 29,  category: 'tshirt',  inStock: true  },
  { id: 3, name: 'Denim Jacket',    price: 99,  category: 'jacket',  inStock: false },
  { id: 4, name: 'Grey Hoodie',     price: 65,  category: 'hoodie',  inStock: true  },
  { id: 5, name: 'Striped Tee',     price: 35,  category: 'tshirt',  inStock: true  },
  { id: 6, name: 'Bomber Jacket',   price: 129, category: 'jacket',  inStock: true  },
  { id: 7, name: 'Oversized Tee',   price: 39,  category: 'tshirt',  inStock: false },
  { id: 8, name: 'Zip-up Hoodie',   price: 79,  category: 'hoodie',  inStock: true  },
];

function createCard(product) {
  var card = document.createElement('div');
  card.className = 'product-card';


  var stockClass = product.inStock ? 'badge-success' : 'badge-danger';
  var stockText  = product.inStock ? 'В наявності' : 'Немає';


  card.innerHTML = `
    <img src='https://via.placeholder.com/250' alt='${product.name}'>
    <div class='card-body'>
      <h3>${product.name}</h3>
      <p class='price'>$${product.price}</p>
      <span class='badge ${stockClass}'>${stockText}</span>
      <button class='btn-cart'
        ${product.inStock ? '' : 'disabled'}
        data-id='${product.id}'>
        Add to Cart
      </button>
    </div>
  `;


  return card;
}

function renderProducts(list) {
  var grid = document.getElementById('productGrid');
  var counter = document.getElementById('counter');


  grid.innerHTML = '';


  if (list.length === 0) {
    grid.innerHTML = '<p class="empty">Товарів не знайдено</p>';
    counter.textContent = 'Знайдено: 0 товарів';
    return;
  }

  list.forEach(function(product) {
    var card = createCard(product);
    grid.appendChild(card);
  });


  counter.textContent = 'Знайдено: ' + list.length + ' товарів';
}

function applyFilters() {
  var category  = document.getElementById('categoryFilter').value;
  var sortOrder = document.getElementById('sortOrder').value;


  var filtered = products.filter(function(p) {
    return category === 'all' || p.category === category;
  });


  filtered.sort(function(a, b) {
    return sortOrder === 'asc' ? a.price - b.price : b.price - a.price;
  });


  return filtered;
}

document.getElementById('categoryFilter').addEventListener('change', function() {
  renderProducts(applyFilters());
});


document.getElementById('sortOrder').addEventListener('change', function() {
  renderProducts(applyFilters());
});


// Ініціалізація: відображаємо всі товари при завантаженні сторінки
renderProducts(products);
