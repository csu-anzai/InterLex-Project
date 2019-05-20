import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {SharedModule} from '../shared/shared.module';
import {HomeComponent} from './home/home.component';
import {AdminComponent} from './admin/admin.component';
import {ButtonModule} from 'primeng/button';
import {LoginComponent} from './login/login.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';
import {CaseEditorComponent} from './case/case-editor.component';
import {CalendarModule} from 'primeng/calendar';
import {EditorModule} from 'primeng/editor';
import {EulawLinkComponent} from './case/eulaw-link.component';
import {DialogModule} from 'primeng/dialog';
import {DropdownModule} from 'primeng/dropdown';
import {TableModule} from 'primeng/table';
import {
  AutoCompleteModule,
  CardModule,
  ChipsModule,
  ConfirmDialogModule,
  InputTextModule,
  OverlayPanelModule,
  TreeModule,
  TreeTableModule
} from 'primeng/primeng';
import {CaseListComponent} from './case/case-list.component';
import {RadioButtonModule} from 'primeng/radiobutton';
import {EurovocTreeComponent} from './case/eurovoc-tree.component';
import {CheckboxModule} from 'primeng/checkbox';
import {EucaseLinkComponent} from './case/eucase-link.component';
import {EulawHrefPipe, EulinkPartPipe} from './case/eulaw-link-parts.pipe';
import {EucaseHrefPipe, EucasePartPipe, EucasePreliminaryRuling} from './case/eucase-link-parts.pipe';
import {RegisterUserComponent} from './admin/register-user/register-user.component';
import {EditUserComponent} from './admin/edit-user/edit-user.component';
import {ResetUserComponent} from './admin/reset-user/reset-user.component';
import {SuggestionsComponent} from './admin/suggestions/suggestions.component';
import {HelpComponent} from "./help/help.component";

@NgModule({
  imports: [
    FormsModule,
    CommonModule,
    SharedModule,
    ButtonModule,
    ReactiveFormsModule,
    RouterModule,
    CalendarModule,
    EditorModule,
    DialogModule,
    DropdownModule,
    TableModule,
    ConfirmDialogModule,
    RadioButtonModule,
    TreeModule,
    OverlayPanelModule,
    InputTextModule,
    RadioButtonModule,
    CheckboxModule,
    CardModule,
    ChipsModule,
    AutoCompleteModule,
    TreeTableModule,
  ],
  declarations: [HomeComponent, AdminComponent, LoginComponent, CaseEditorComponent,
    EulawLinkComponent, CaseListComponent, EurovocTreeComponent, EucaseLinkComponent,
    EulinkPartPipe, EucasePartPipe, RegisterUserComponent, EulawHrefPipe,
    EucasePreliminaryRuling, EucaseHrefPipe, EditUserComponent, ResetUserComponent, SuggestionsComponent,HelpComponent
  ],
})
export class FeaturesModule {
}
