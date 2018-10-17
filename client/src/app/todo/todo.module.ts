import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule, Type } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { LoginModule } from '../login/login.module';
import { RealtimeModule } from '../realtime/realtime.module';
import { UiModule } from '../ui/ui.module';
import { ItemsComponent } from './components/items/items.component';
import { ListComponent } from './components/list/list.component';
import { TodoModuleConfiguration } from './models/todo-module-configuration';
import { ItemsService } from './services/items.service';
import { ListService } from './services/list.service';
import { TodoRoutingModule } from './todo-routing.module';

@NgModule({
  declarations: [ListComponent, ItemsComponent],
  imports: [TodoRoutingModule, LoginModule, CommonModule, UiModule, RouterModule, ReactiveFormsModule, RealtimeModule],
})
export class TodoModule {
  static forRoot(moduleConfiguration: Type<TodoModuleConfiguration>): ModuleWithProviders {
    return {
      ngModule: TodoModule,
      providers: [
        ListService,
        ItemsService,
        {
          provide: TodoModuleConfiguration,
          useClass: moduleConfiguration,
        },
      ],
    };
  }
}
