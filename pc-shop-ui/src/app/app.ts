import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { CartService } from './core/services/cart.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  public cartService = inject(CartService);

  ngOnInit() {
    this.cartService.loadCart();
  }

  get cartItemsCount() {
    const cart = this.cartService.cartState();
    if (!cart) return 0;
    return cart.items.reduce((acc, item) => acc + item.quantity, 0);
  }
}
