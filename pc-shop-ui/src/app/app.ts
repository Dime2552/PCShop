import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { CartService } from './core/services/cart.service';
import { ToastModule } from 'primeng/toast';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, ToastModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  public cartService = inject(CartService);
  public authService = inject(AuthService);

  ngOnInit() {
    this.cartService.loadCart();
  }

  get cartItemsCount() {
    const cart = this.cartService.cartState();
    if (!cart) return 0;
    return cart.items.reduce((acc, item) => acc + item.quantity, 0);
  }

  logout() {
    this.authService.logout();
    this.cartService.loadCart();
  }
}
