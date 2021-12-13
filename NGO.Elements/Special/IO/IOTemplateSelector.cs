using System.Windows;
using System.Windows.Controls;

namespace NGO.Elements.Special.IO
{
    public class IOTemplateSelector : DataTemplateSelector
    {

        public DataTemplate discreteTemplate { get; set; }
        public DataTemplate analogTemplate { get; set; }
        public DataTemplate counterTemplate { get; set; }
        public DataTemplate failureTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var io = item as InputOutput;
            if (io == null) return null;

            // Со времен 153 заказа, будем считать что есть только следующие типы входов/выходов
            // дискретный, аналоговый и счетчик испульсов 
            // (а не как в раньше: дискретный, аналоговый старый, аналоговый новый aka сургутский, импульсы)
            // хитрость в том, что при наличии в файлах записи типа {0:0.###} mAmps = {1:0.#} atm.
            // а также динамической адресации в ОРС сервере,
            // можно в качестве counterTemplate использовать analogTemplate
            // Но подобное чудо-подставление будет сделано в стилях...
            // типа "оставили на совести лютых дизайнеров"

            if (io.tag != null)
                switch (io.viewTemplateType)
                {
                    case ViewTemplateType.Discrete:
                        return discreteTemplate;

                    case ViewTemplateType.Analog:
                        return analogTemplate;

                    case ViewTemplateType.Counter:
                        return counterTemplate;

                    default:
                        return failureTemplate;
                }

            return failureTemplate;
        }
    }
}
