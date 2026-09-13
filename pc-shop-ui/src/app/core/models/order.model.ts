export interface OrderItemDto {
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
}

export interface OrderDto {
  orderId: string;
  totalAmount: number;
  shippingCost: number;
  status: string;
  createdAt: string;
  items: OrderItemDto[];
}
