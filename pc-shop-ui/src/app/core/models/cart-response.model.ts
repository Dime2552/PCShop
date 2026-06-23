import { CartItemDto } from "./cart-item.model";

export interface CartResponseDto {
  cartId: string;
  items: CartItemDto[];
  totalCartPrice: number;
}