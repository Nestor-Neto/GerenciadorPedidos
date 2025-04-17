using AutoMapper;
using GerenciadorPedidos.Domain.Entities.DataTransferObjects;
using GerenciadorPedidos.Domain.Entities.Objects;

namespace GerenciadorPedidos.Domain.Mappers
{
    public class AutoMapperSetup : Profile
    {
        public AutoMapperSetup()
        {
            // Mapeamento entre Pedido e DTOs
            CreateMap<Pedido, PedidoResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PedidoId, opt => opt.MapFrom(src => src.PedidoId))
                .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.ClienteId))
                .ForMember(dest => dest.Imposto, opt => opt.MapFrom(src => src.Imposto))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens));

            CreateMap<Pedido, PedidoCriadoDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // Mapeamento entre PedidoItem e DTOs
            CreateMap<PedidoItem, PedidoItemDTO>()
                .ForMember(dest => dest.ProdutoId, opt => opt.MapFrom(src => src.ProdutoId))
                .ForMember(dest => dest.Quantidade, opt => opt.MapFrom(src => src.Quantidade))
                .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor));

            CreateMap<PedidoItemDTO, PedidoItem>()
                .ForMember(dest => dest.ProdutoId, opt => opt.MapFrom(src => src.ProdutoId))
                .ForMember(dest => dest.Quantidade, opt => opt.MapFrom(src => src.Quantidade))
                .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor));

            // Mapeamento de NovoPedidoDTO para Pedido
            CreateMap<NovoPedidoDTO, Pedido>()
                .ForMember(dest => dest.PedidoId, opt => opt.MapFrom(src => src.PedidoId))
                .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.ClienteId))
                .ForMember(dest => dest.Itens, opt => opt.Ignore())
                .AfterMap((src, dest) => {
                    foreach (var itemDto in src.Itens)
                    {
                        var item = new PedidoItem(itemDto.ProdutoId, itemDto.Quantidade, itemDto.Valor);
                        dest.AdicionarItem(item);
                    }
                });
        }
    }
}
