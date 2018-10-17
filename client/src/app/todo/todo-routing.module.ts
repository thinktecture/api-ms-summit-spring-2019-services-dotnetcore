import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IsAuthenticatedGuard } from '../login/services/is-authenticated.guard';
import { ItemsComponent } from './components/items/items.component';
import { ListComponent } from './components/list/list.component';

const routes: Routes = [
  {
    path: 'todo',
    canActivateChild: [IsAuthenticatedGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'list',
      },
      {
        path: 'list',
        children: [
          {
            path: '',
            pathMatch: 'full',
            component: ListComponent,
          },
          {
            path: ':id',
            component: ItemsComponent,
          },
        ],
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
})
export class TodoRoutingModule {

}
