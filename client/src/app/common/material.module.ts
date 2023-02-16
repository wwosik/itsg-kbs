import { NgModule } from '@angular/core';
import { CommonModule, registerLocaleData } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTableModule } from '@angular/material/table';
import { MatStepperModule } from '@angular/material/stepper';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';
import { MatSortModule } from '@angular/material/sort';
import { MatRadioModule } from '@angular/material/radio';
import { MatDateFnsModule } from '@angular/material-date-fns-adapter';


@NgModule({
	declarations: [],
	imports: [
		CommonModule,
		MatButtonModule,
		MatFormFieldModule,
		MatInputModule,
		MatToolbarModule,
		MatTooltipModule,
		MatTabsModule,
		MatCheckboxModule,
		MatTableModule,
		MatStepperModule,
		MatProgressSpinnerModule,
		MatSnackBarModule,
		MatDatepickerModule,
		MatDialogModule,
		MatDateFnsModule,
		MatSelectModule,
		MatSortModule,
		MatRadioModule,
	],
	exports: [
		MatButtonModule,
		MatFormFieldModule,
		MatInputModule,
		MatToolbarModule,
		MatTooltipModule,
		MatTabsModule,
		MatCheckboxModule,
		MatTableModule,
		MatStepperModule,
		MatProgressSpinnerModule,
		MatSnackBarModule,
		MatDatepickerModule,
		MatDialogModule,
		MatDateFnsModule,
		MatSelectModule,
		MatSortModule,
		MatRadioModule,
	]	
})
export class MaterialModule {}
