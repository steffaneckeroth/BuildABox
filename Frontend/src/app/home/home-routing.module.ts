import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {ProductsComponent} from "../products/products.component";
import {CreateBoxComponent} from "../create-box/create-box.component";
import {DetailsComponent} from "../details/details.component";
import {UpdateBoxComponent} from "../update-box/update-box.component";

const routes: Routes = [
  {
    path: '',
    component: ProductsComponent,
  },
  {path: 'products', component: ProductsComponent},
  {path: 'createBox', component: CreateBoxComponent},
  {path: 'details/:id', component: DetailsComponent},
  {path: 'edit', component: UpdateBoxComponent},

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomePageRoutingModule {
}
