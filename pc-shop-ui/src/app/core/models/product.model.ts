export interface ProductDto {
  id: string;
  name: string;
  brand: string;
  price: number;
  discountPrice?: number;
  mainImageUrl: string;
  stockQuantity: number;
}